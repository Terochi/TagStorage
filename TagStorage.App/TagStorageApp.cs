using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Screens;
using osuTK;

namespace TagStorage.App
{
    public partial class TagStorageApp : TagStorageAppBase
    {
        private ScreenStack screenStack;

        [BackgroundDependencyLoader]
        private void load()
        {
            // Add your top-level game components here.
            // A screen stack and sample screen has been provided for convenience, but you can replace it if you don't want to use screens.
            Child = new GridContainer
            {
                RelativeSizeAxes = Axes.Both,
                ColumnDimensions = [new Dimension(GridSizeMode.AutoSize), new Dimension()],
                RowDimensions = [new Dimension()],
                Content = new Drawable[][]
                {
                    [
                        new Container
                        {
                            RelativeSizeAxes = Axes.Y,
                            Width = 200,
                            Children =
                            [
                                new Box
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Colour = Colour4.DimGray,
                                },
                                new FillFlowContainer
                                {
                                    RelativeSizeAxes = Axes.Both,
                                    Direction = FillDirection.Vertical,
                                    Spacing = new Vector2(20),
                                    Children =
                                    [
                                        new BasicButton
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            Height = 100,
                                            Text = "Main screen",
                                            Action = () =>
                                            {
                                                screenStack.Push(new MainScreen());
                                            }
                                        },
                                        new BasicButton
                                        {
                                            RelativeSizeAxes = Axes.X,
                                            Height = 100,
                                            Text = "Tag screen",
                                            Action = () =>
                                            {
                                                screenStack.Push(new TagScreen());
                                            }
                                        }
                                    ]
                                }
                            ]
                        },
                        screenStack =
                            new ScreenStack
                            {
                                RelativeSizeAxes = Axes.Both
                            }
                    ]
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            screenStack.Push(new MainScreen());
        }
    }
}
